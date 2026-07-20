import { useEffect, useState } from "react";
import { getProducts, type Product } from "./api";
import "./App.css";

/**
 * Products page. Demonstrates the core frontend-integration pattern:
 *  - call the API in an effect (on mount),
 *  - track three pieces of state: loading, error, data.
 */
function App() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getProducts()
      .then(setProducts)
      .catch((err: unknown) => setError(err instanceof Error ? err.message : "Unknown error"))
      .finally(() => setLoading(false));
  }, []); // empty deps => run once on mount

  return (
    <main className="container">
      <h1>🛍️ Products</h1>
      <p className="subtitle">Served by ProductsApi at <code>http://localhost:5182/v1/products</code></p>

      {loading && <p className="muted">Loading…</p>}
      {error && <p className="error">⚠️ {error}</p>}

      {!loading && !error && (
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th className="right">Price</th>
              <th className="right">Category</th>
            </tr>
          </thead>
          <tbody>
            {products.map((product) => (
              <tr key={product.id}>
                <td>{product.id}</td>
                <td>{product.name}</td>
                <td className="right">${product.price.toFixed(2)}</td>
                <td className="right">{product.categoryId}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {!loading && !error && products.length === 0 && <p className="muted">No products yet.</p>}
    </main>
  );
}

export default App;
