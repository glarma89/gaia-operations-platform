import { useEffect, useState } from "react";
import { getOperations } from "./api/operationsApi";
import { OperationSelector } from "./components/OperationSelector";
import type { OperationDefinition } from "./types/operations";
import { OperationForm } from "./components/OperationForm";
import "./App.css";

function App() {
  const [operations, setOperations] = useState<OperationDefinition[]>([]);
  const [selectedOperationId, setSelectedOperationId] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let isActive = true;

    async function loadOperations() {
      try {
        const data = await getOperations();

        if (!isActive) return;

        setOperations(data);

        if (data.length > 0) {
          setSelectedOperationId(data[0].id);
        }
      } catch (err) {
        if (!isActive) return;

        setError(
          err instanceof Error ? err.message : "Failed to load operations.",
        );
      } finally {
        if (isActive) {
          setIsLoading(false);
        }
      }
    }

    void loadOperations();

    return () => {
      isActive = false;
    };
  }, []);

  const selectedOperation = operations.find(
    (operation) => operation.id === selectedOperationId,
  );

  return (
    <main className="app">
      <header className="app-header">
        <h1>Gaia Operations Platform</h1>
        <p>Extensible operations engine</p>
      </header>

      <section className="app-content">
        <h2>Operations</h2>

        {isLoading && <p>Loading operations...</p>}

        {error && (
          <p className="error-message" role="alert">
            {error}
          </p>
        )}

        {!isLoading && !error && operations.length === 0 && (
          <p>No operations available.</p>
        )}

        {!isLoading && !error && operations.length > 0 && (
          <>
            <p>Select an operation:</p>

            <OperationSelector
              operations={operations}
              selectedOperationId={selectedOperationId}
              onSelect={setSelectedOperationId}
            />

            {selectedOperation && (
              <OperationForm
                key={selectedOperation.id}
                operation={selectedOperation}
              />
            )}

            <p className="selection-info">
              Selected operation: <strong>{selectedOperationId}</strong>
            </p>
          </>
        )}
      </section>
    </main>
  );
}

export default App;
