
import type { OperationDefinition } from '../types/operations'

interface OperationSelectorProps {
  operations: OperationDefinition[]
  selectedOperationId: string
  onSelect: (operationId: string) => void
}

export function OperationSelector({
  operations,
  selectedOperationId,
  onSelect,
}: OperationSelectorProps) {
  return (
    <div className="operation-grid">
      {operations.map((operation) => (
        <button
          key={operation.id}
          type="button"
          className={
            selectedOperationId === operation.id
              ? 'operation-card selected'
              : 'operation-card'
          }
          onClick={() => onSelect(operation.id)}
          aria-pressed={selectedOperationId === operation.id}
        >
          <strong>{operation.name}</strong>
          <span>{operation.description}</span>
        </button>
      ))}
    </div>
  )
}
