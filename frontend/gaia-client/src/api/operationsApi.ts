
import type {
  ApiProblem,
  ExecuteOperationRequest,
  OperationDefinition,
  OperationHistoryResponse,
  OperationResult
} from '../types/operations'

const API_BASE_URL = '/api/operations'


export class ApiError extends Error {
  public readonly status: number

  constructor(status: number, message: string) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}


async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    let message = `HTTP error ${response.status}`

    try {
      const problem: ApiProblem = await response.json()
      message = problem.detail || problem.title || message
    } catch {
      // Keep the fallback HTTP error message.
    }

    throw new ApiError(response.status, message)
  }

  return (await response.json()) as T
}

export async function getOperations(): Promise<OperationDefinition[]> {
  const response = await fetch(API_BASE_URL)

  return handleResponse<OperationDefinition[]>(response)
}

export async function executeOperation(
  request: ExecuteOperationRequest,
): Promise<OperationResult> {
  const response = await fetch(`${API_BASE_URL}/execute`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  })

  return handleResponse<OperationResult>(response)
}


export async function getOperationHistory(
  operationId: string,
): Promise<OperationHistoryResponse> {
  const response = await fetch(
    `${API_BASE_URL}/${encodeURIComponent(operationId)}/history`,
  )

  return handleResponse<OperationHistoryResponse>(response)
}
