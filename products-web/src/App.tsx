import { useState, type FormEvent } from "react";
import { useAuth, type DevPersona } from "./auth";
import { useCategories, useCreateProduct, useDeleteProduct, useProducts } from "./hooks";
import { ApiError } from "./api";
import "./App.css";

/**
 * Products page. Demonstrates the full frontend-integration pattern:
 *  - server state via TanStack Query (useQuery for reads, useMutation for writes),
 *  - auth token handling (Bearer) with UI gated on scopes/roles,
 *  - 401/403 handling surfaced as friendly messages.
 */
function App() {
  return (
    <main className="container">
      <h1>🛍️ Products</h1>
      <p className="subtitle">
        Served by ProductsApi at <code>http://localhost:5182/v1/products</code>
      </p>
      <AuthBar />
      <CreateProductForm />
      <ProductList />
    </main>
  );
}

/** Dev sign-in bar: mints a writer or admin token, or shows who's signed in. */
function AuthBar() {
  const { user, isAdmin, signIn, signOut } = useAuth();
  const [error, setError] = useState<string | null>(null);

  async function handleSignIn(persona: DevPersona) {
    setError(null);
    try {
      await signIn(persona);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Sign-in failed");
    }
  }

  return (
    <section className="authbar">
      {user ? (
        <>
          <span className="muted">
            Signed in as <strong>{user.name}</strong>
            {isAdmin ? " (admin)" : user.scopes.includes("products.write") ? " (writer)" : ""}
          </span>
          <button onClick={signOut}>Sign out</button>
        </>
      ) : (
        <>
          <span className="muted">Not signed in — reads are public; writes need a token.</span>
          <button onClick={() => handleSignIn("writer")}>Sign in as writer</button>
          <button onClick={() => handleSignIn("admin")}>Sign in as admin</button>
        </>
      )}
      {error && <p className="error">⚠️ {error}</p>}
    </section>
  );
}

/** Create form, shown only when the current token allows writes (products.write). */
function CreateProductForm() {
  const { canWrite } = useAuth();
  const categories = useCategories();
  const createProduct = useCreateProduct();
  const [name, setName] = useState("");
  const [price, setPrice] = useState("");
  const [categoryId, setCategoryId] = useState("");

  if (!canWrite) return null;

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    createProduct.mutate(
      { name, price: Number(price), categoryId: Number(categoryId) },
      {
        onSuccess: () => {
          setName("");
          setPrice("");
          setCategoryId("");
        },
      },
    );
  }

  return (
    <section className="card">
      <h2>Add a product</h2>
      <form className="form" onSubmit={handleSubmit}>
        <input
          className="grow"
          placeholder="Name"
          value={name}
          maxLength={50}
          required
          onChange={(event) => setName(event.target.value)}
        />
        <input
          type="number"
          placeholder="Price"
          value={price}
          min={0.01}
          max={100000}
          step={0.01}
          required
          onChange={(event) => setPrice(event.target.value)}
        />
        <select
          value={categoryId}
          required
          onChange={(event) => setCategoryId(event.target.value)}
        >
          <option value="" disabled>
            {categories.isPending ? "Loading categories…" : "Select a category"}
          </option>
          {categories.data?.map((category) => (
            <option key={category.id} value={category.id}>
              {category.name}
            </option>
          ))}
        </select>
        <button type="submit" disabled={createProduct.isPending}>
          {createProduct.isPending ? "Adding…" : "Add"}
        </button>
      </form>
      {createProduct.isError && <p className="error">⚠️ {errorMessage(createProduct.error)}</p>}
    </section>
  );
}

/** The product table. Delete buttons appear only for admins. */
function ProductList() {
  const { isAdmin } = useAuth();
  const { data: products, isPending, isError, error } = useProducts();
  const deleteProduct = useDeleteProduct();

  if (isPending) return <p className="muted">Loading…</p>;
  if (isError) return <p className="error">⚠️ {errorMessage(error)}</p>;
  if (products.length === 0) return <p className="muted">No products yet.</p>;

  return (
    <>
      {deleteProduct.isError && <p className="error">⚠️ {errorMessage(deleteProduct.error)}</p>}
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Name</th>
            <th className="right">Price</th>
            <th className="right">Category</th>
            {isAdmin && <th className="right">Actions</th>}
          </tr>
        </thead>
        <tbody>
          {products.map((product) => (
            <tr key={product.id}>
              <td>{product.id}</td>
              <td>{product.name}</td>
              <td className="right">${product.price.toFixed(2)}</td>
              <td className="right">{product.categoryId}</td>
              {isAdmin && (
                <td className="right">
                  <button
                    className="danger"
                    disabled={deleteProduct.isPending}
                    onClick={() => deleteProduct.mutate(product.id)}
                  >
                    Delete
                  </button>
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
}

/** Turn an error into a friendly message, mapping 401/403 to auth guidance. */
function errorMessage(error: unknown): string {
  if (error instanceof ApiError) {
    if (error.status === 401) return "You need to sign in to do that (401).";
    if (error.status === 403) return "You don't have permission for that (403).";
    return error.message;
  }
  return error instanceof Error ? error.message : "Unknown error";
}

export default App;
