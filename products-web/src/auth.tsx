// Auth abstraction for the frontend.
//
// DESIGN: This is a provider-agnostic seam. Components only ever consume `useAuth()`,
// so swapping the DEV token minter for real Microsoft Entra ID (MSAL) later is a
// single-file change here — no component needs to know which identity provider is used.
//
// Today it uses the API's DEV-ONLY `GET /dev/token` endpoint (see products-api
// Endpoints/DevEndpoints.cs). To move to Entra ID:
//   1. npm i @azure/msal-browser @azure/msal-react
//   2. reimplement AuthProvider with MsalProvider + useMsal().acquireTokenSilent({ scopes })
//   3. flip the API to options.Authority / options.Audience (see ServiceCollectionExtensions.cs)
// The `AuthContextValue` contract below stays identical, so the rest of the app is untouched.

import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useRef,
  useState,
  type ReactNode,
} from "react";

const API_BASE = "http://localhost:5182";

/** The signed-in user, derived from the JWT claims. `null` when anonymous. */
export interface AuthUser {
  name: string;
  scopes: string[]; // e.g. ["products.write"] — delegated permissions (what the app may do)
  roles: string[]; // e.g. ["admin"]            — app roles (who the user is)
}

/** Two dev personas, mapped to the claims each protected endpoint requires. */
export type DevPersona = "writer" | "admin";

export interface AuthContextValue {
  user: AuthUser | null;
  /** Whether the current token satisfies the API's `products.write` policy (POST/PUT). */
  canWrite: boolean;
  /** Whether the current token satisfies the API's `admin` policy (DELETE). */
  isAdmin: boolean;
  /** Acquire a token for the chosen persona (DEV: mints one via /dev/token). */
  signIn: (persona: DevPersona) => Promise<void>;
  signOut: () => void;
  /** Returns the current access token (async to mirror MSAL's acquireTokenSilent). */
  getAccessToken: () => Promise<string | null>;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

/** Decode a JWT payload (base64url) without a dependency. Returns {} on any problem. */
function decodeJwt(token: string): Record<string, unknown> {
  try {
    const payload = token.split(".")[1];
    const json = atob(payload.replace(/-/g, "+").replace(/_/g, "/"));
    return JSON.parse(json) as Record<string, unknown>;
  } catch {
    return {};
  }
}

/** Normalize a claim that may be a space-delimited string or an array into string[]. */
function toStringArray(claim: unknown): string[] {
  if (Array.isArray(claim)) return claim.map(String);
  if (typeof claim === "string") return claim.split(" ").filter(Boolean);
  return [];
}

/** Build an AuthUser from the token's claims, tolerating both dev and Entra claim shapes. */
function userFromToken(token: string): AuthUser {
  const claims = decodeJwt(token);
  return {
    name: (claims.sub as string) ?? "unknown",
    // Entra uses "scp"; our dev token uses "scope". Support both.
    scopes: toStringArray(claims.scope ?? claims.scp),
    // Entra uses "roles" (array); our dev token uses "role" (string). Support both.
    roles: toStringArray(claims.role ?? claims.roles),
  };
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(null);
  // The token lives in memory only (a ref) — never localStorage — so an XSS can't read it
  // from storage and it's naturally cleared on refresh. Fine for a dev/learning app.
  const tokenRef = useRef<string | null>(null);

  const signIn = useCallback(async (persona: DevPersona) => {
    const response = await fetch(`${API_BASE}/dev/token?admin=${persona === "admin"}`);
    if (!response.ok) throw new Error(`Sign-in failed (HTTP ${response.status})`);
    const { access_token } = (await response.json()) as { access_token: string };
    tokenRef.current = access_token;
    setUser(userFromToken(access_token));
  }, []);

  const signOut = useCallback(() => {
    tokenRef.current = null;
    setUser(null);
  }, []);

  const getAccessToken = useCallback(async () => tokenRef.current, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      canWrite: user?.scopes.includes("products.write") ?? false,
      isAdmin: user?.roles.includes("admin") ?? false,
      signIn,
      signOut,
      getAccessToken,
    }),
    [user, signIn, signOut, getAccessToken],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

/** Consume the auth context. Throws if used outside <AuthProvider>. */
export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (context === undefined) throw new Error("useAuth must be used within <AuthProvider>");
  return context;
}
