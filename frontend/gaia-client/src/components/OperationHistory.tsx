
import { useEffect, useState } from 'react'
import { getOperationHistory } from '../api/operationsApi'
import type { OperationHistoryResponse } from '../types/operations'

interface OperationHistoryProps {
  operationId: string
  refreshKey: number
}

export function OperationHistory({
  operationId,
  refreshKey,
}: OperationHistoryProps) {
  const [history, setHistory] = useState<OperationHistoryResponse | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let isActive = true

    async function loadHistory() {
      setIsLoading(true)
      setError(null)

      try {
        const data = await getOperationHistory(operationId)

        if (isActive) {
          setHistory(data)
        }
      } catch (err) {
        if (isActive) {
          setError(
            err instanceof Error
              ? err.message
              : 'Failed to load operation history.',
          )
          setHistory(null)
        }
      } finally {
        if (isActive) {
          setIsLoading(false)
        }
      }
    }

    void loadHistory()

    return () => {
      isActive = false
    }
  }, [operationId, refreshKey])

  return (
    <section className="operation-history">
      <h3>Operation history</h3>

      {isLoading && <p>Loading history...</p>}

      {error && (
        <p className="error-message" role="alert">
          {error}
        </p>
      )}

      {!isLoading && !error && history && (
        <>
          <p className="monthly-count">
            Executions this month: <strong>{history.monthlyCount}</strong>
          </p>

          <h4>Latest executions</h4>

          {history.latestExecutions.length === 0 ? (
            <p>No executions yet.</p>
          ) : (
            <ul className="history-list">
              {history.latestExecutions.map((item, index) => (
                <li key={`${item.executedAtUtc}-${index}`}>
                  <div>
                    <strong>Input A:</strong> {item.inputA}
                  </div>
                  <div>
                    <strong>Input B:</strong> {item.inputB}
                  </div>
                  <div>
                    <strong>Result:</strong> {String(item.result)}
                  </div>
                  <time dateTime={item.executedAtUtc}>
                    {new Date(item.executedAtUtc).toLocaleString()}
                  </time>
                </li>
              ))}
            </ul>
          )}
        </>
      )}
    </section>
  )
}
