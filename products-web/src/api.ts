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

/**
 * GET /v1/products — public endpoint (no auth required).
 * Throws on non-2xx so the caller can show an error state.
 */
export async function getProducts(): Promise<Product[]> {
  const response = await fetch(`${API_BASE}/v1/products`);
  if (!response.ok) {
    throw new Error(`Failed to load products (HTTP ${response.status})`);
  }
  return (await response.json()) as Product[];
}
