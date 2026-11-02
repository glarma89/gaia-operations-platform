
import { useRef, useState } from 'react'
import { executeOperation } from '../api/operationsApi'
import type {
  OperationDefinition,
  OperationResult,
} from '../types/operations'

interface OperationFormProps {
  operation: OperationDefinition
}

export function OperationForm({ operation }: OperationFormProps) {
  const [inputA, setInputA] = useState('')
  const [inputB, setInputB] = useState('')
  const [result, setResult] = useState<OperationResult | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isExecuting, setIsExecuting] = useState(false)

  const requestId = useRef(0)

  const isNumeric = operation.inputType === 'number'

  function clearFeedback() {
    requestId.current += 1
    setResult(null)
    setError(null)
    setIsExecuting(false)
  }

  function handleInputAChange(value: string) {
    setInputA(value)
    clearFeedback()
  }

  function handleInputBChange(value: string) {
    setInputB(value)
    clearFeedback()
  }

  async function handleSubmit(event: React.SubmitEvent<HTMLFormElement>) {
    event.preventDefault()

    if (isExecuting) return

    if (isNumeric && (!inputA.trim() || !inputB.trim())) {
      setError('Both numeric values are required.')
      return
    }

    const currentRequestId = ++requestId.current

    setIsExecuting(true)
    setError(null)
    setResult(null)

    try {
      const data = await executeOperation({
        operationId: operation.id,
        inputA,
        inputB,
      })

      if (currentRequestId === requestId.current) {
        setResult(data)
      }
    } catch (err) {
      if (currentRequestId === requestId.current) {
        setError(
          err instanceof Error
            ? err.message
            : 'Failed to execute operation.',
        )
      }
    } finally {
      if (currentRequestId === requestId.current) {
        setIsExecuting(false)
      }
    }
  }

  return (
    <form className="operation-form" onSubmit={handleSubmit}>
      <div className="form-heading">
        <h3>Execute operation</h3>
        <p>{operation.name}</p>
      </div>

      <div className="form-inputs">
        <div className="input-group">
          <label htmlFor="input-a">First value</label>
          <input
            id="input-a"
            type={isNumeric ? 'number' : 'text'}
            step={isNumeric ? 'any' : undefined}
            value={inputA}
            onChange={(event) => handleInputAChange(event.target.value)}
            required={isNumeric}
            placeholder={isNumeric ? 'Enter a number' : 'Enter text'}
          />
        </div>

        <div className="input-group">
          <label htmlFor="input-b">Second value</label>
          <input
            id="input-b"
            type={isNumeric ? 'number' : 'text'}
            step={isNumeric ? 'any' : undefined}
            value={inputB}
            onChange={(event) => handleInputBChange(event.target.value)}
            required={isNumeric}
            placeholder={isNumeric ? 'Enter a number' : 'Enter text'}
          />
        </div>
      </div>

      <button
        className="execute-button"
        type="submit"
        disabled={isExecuting}
      >
        {isExecuting ? 'Executing...' : 'Execute operation'}
      </button>

      {error && (
        <div className="error-message" role="alert">
          {error}
        </div>
      )}

      {result && (
        <div className="result-box" role="status">
          <span className="result-label">Result</span>
          <strong className="result-value">
            {String(result.value)}
          </strong>
        </div>
      )}
    </form>
  )
}
