
import { useState } from 'react'
import { executeOperation } from '../api/operationsApi'
import type { OperationDefinition, OperationResult } from '../types/operations'

interface OperationFormProps {
  operation: OperationDefinition
}

export function OperationForm({ operation }: OperationFormProps) {
  const [inputA, setInputA] = useState('')
  const [inputB, setInputB] = useState('')
  const [result, setResult] = useState<OperationResult | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isExecuting, setIsExecuting] = useState(false)

  async function handleSubmit(event: React.SubmitEvent<HTMLFormElement>) {
    event.preventDefault()

    setIsExecuting(true)
    setError(null)
    setResult(null)

    try {
      const data = await executeOperation({
        operationId: operation.id,
        inputA,
        inputB,
      })

      setResult(data)
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : 'Failed to execute operation.',
      )
    } finally {
      setIsExecuting(false)
    }
  }

  const isNumeric = operation.inputType === 'number'

  return (
    <form className="operation-form" onSubmit={handleSubmit}>
      <h3>Execute: {operation.name}</h3>

      <div className="input-group">
        <label htmlFor="input-a">First value</label>
        <input
          id="input-a"
          type={isNumeric ? 'number' : 'text'}
          step={isNumeric ? 'any' : undefined}
          value={inputA}
          onChange={(event) => setInputA(event.target.value)}
          required={isNumeric}
        />
      </div>

      <div className="input-group">
        <label htmlFor="input-b">Second value</label>
        <input
          id="input-b"
          type={isNumeric ? 'number' : 'text'}
          step={isNumeric ? 'any' : undefined}
          value={inputB}
          onChange={(event) => setInputB(event.target.value)}
          required={isNumeric}
        />
      </div>

      <button type="submit" disabled={isExecuting}>
        {isExecuting ? 'Executing...' : 'Execute'}
      </button>

      {error && (
        <p className="error-message" role="alert">
          {error}
        </p>
      )}

      {result && (
        <div className="result-box" role="status">
          <strong>Result:</strong> {String(result.value)}
        </div>
      )}
    </form>
  )
}
