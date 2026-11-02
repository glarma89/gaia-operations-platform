
export interface OperationDefinition {
  id: string
  name: string
  description: string
  inputType: string
}

export interface ExecuteOperationRequest {
  operationId: string
  inputA: string
  inputB: string
}

export interface OperationResult {
  operationId: string
  value: number | string | boolean
}

export interface ApiProblem {
  type?: string
  title?: string
  status?: number
  detail?: string
  instance?: string
}

export interface OperationHistoryItem {
  inputA: string
  inputB: string
  result: number | string | boolean
  executedAtUtc: string
}

export interface OperationHistoryResponse {
  operationId: string
  latestExecutions: OperationHistoryItem[]
  monthlyCount: number
}
