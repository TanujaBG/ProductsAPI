// Typed API client for the ProductsApi backend.
// In a real app the base URL comes from config (e.g. import.meta.env.VITE_API_BASE),
// so the same code works against localhost, staging, and production.
const API_BASE = "http://localhost:5182";

/** Matches the JSON shape returned by GET /v1/products. */
export interface Product {
  id: number;
  name: string;
  price: number;
  description?: string | null;
  categoryId: number;
}

/** Matches the JSON shape returned by GET /v1/categories. */
export interface Category {
  id: number;
  name: string;
  productCount: number;
}

/** Body the API expects for POST/PUT /v1/products (the server assigns the id). */
export interface CreateProductRequest {
  name: string;
  price: number;
  categoryId: number;
}

/**
 * Error carrying the HTTP status so the UI can tell apart
 * "sign in" (401) from "not allowed" (403) from everything else.
 */
export class ApiError extends Error {
  readonly status: number;

  constructor(status: number, message: string) {
    super(message);
    this.name = "ApiError";
    this.status = status;
  }
}

/** Turn a non-2xx response into an ApiError, preferring the ProblemDetails detail/title. */
async function throwForStatus(response: Response): Promise<never> {
  let detail = `HTTP ${response.status}`;
  try {
    const problem = (await response.json()) as { detail?: string; title?: string };
    detail = problem.detail ?? problem.title ?? detail;
  } catch {
    // Body wasn't JSON (e.g. an empty 401 challenge) — keep the default message.
  }
  throw new ApiError(response.status, detail);
}

/** Attach the Bearer token (when present) to a request's headers. */
function authHeaders(token: string | null): HeadersInit {
  return token ? { Authorization: `Bearer ${token}` } : {};
}

/**
 * GET /v1/products — public endpoint (no auth required).
 * Throws on non-2xx so the caller can show an error state.
 */
export async function getProducts(): Promise<Product[]> {
  const response = await fetch(`${API_BASE}/v1/products`);
  if (!response.ok) return throwForStatus(response);
  return (await response.json()) as Product[];
}

/** GET /v1/categories — public; used to populate the create form's dropdown. */
export async function getCategories(): Promise<Category[]> {
  const response = await fetch(`${API_BASE}/v1/categories`);
  if (!response.ok) return throwForStatus(response);
  return (await response.json()) as Category[];
}

/** POST /v1/products — requires the products.write scope (Bearer token). */
export async function createProduct(
  request: CreateProductRequest,
  token: string | null,
): Promise<Product> {
  const response = await fetch(`${API_BASE}/v1/products`, {
    method: "POST",
    headers: { "Content-Type": "application/json", ...authHeaders(token) },
    body: JSON.stringify(request),
  });
  if (!response.ok) return throwForStatus(response);
  return (await response.json()) as Product;
}

/** DELETE /v1/products/{id} — requires the admin role (Bearer token). */
export async function deleteProduct(id: number, token: string | null): Promise<void> {
  const response = await fetch(`${API_BASE}/v1/products/${id}`, {
    method: "DELETE",
    headers: authHeaders(token),
  });
  if (!response.ok) return throwForStatus(response);
}
