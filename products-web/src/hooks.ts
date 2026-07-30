// TanStack Query hooks — the app's "server state" layer.
//
// Reads use useQuery (cached under a query key); writes use useMutation and then
// invalidate the ["products"] key so the list refetches itself automatically.
// Mutations pull the access token from useAuth and hand it to the API client.

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createProduct,
  deleteProduct,
  getCategories,
  getProducts,
  type CreateProductRequest,
} from "./api";
import { useAuth } from "./auth";

// Query keys = cache identity. Keep them in one place so reads and invalidations agree.
const PRODUCTS_KEY = ["products"] as const;
const CATEGORIES_KEY = ["categories"] as const;

/** Read the product list. Cached under ["products"] and refetched when invalidated. */
export function useProducts() {
  return useQuery({ queryKey: PRODUCTS_KEY, queryFn: getProducts });
}

/** Read categories (for the create form's dropdown). */
export function useCategories() {
  return useQuery({ queryKey: CATEGORIES_KEY, queryFn: getCategories });
}

/** Create a product, then invalidate ["products"] so the list refetches automatically. */
export function useCreateProduct() {
  const queryClient = useQueryClient();
  const { getAccessToken } = useAuth();
  return useMutation({
    mutationFn: async (request: CreateProductRequest) =>
      createProduct(request, await getAccessToken()),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: PRODUCTS_KEY }),
  });
}

/** Delete a product (admin only), then invalidate ["products"]. */
export function useDeleteProduct() {
  const queryClient = useQueryClient();
  const { getAccessToken } = useAuth();
  return useMutation({
    mutationFn: async (id: number) => deleteProduct(id, await getAccessToken()),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: PRODUCTS_KEY }),
  });
}
