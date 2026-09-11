// Shared shapes between the SPA and the customer API.
// NOTE: mirrors the C# models - deliberately missing lastName (demo scenario 1).

export interface Customer {
  id: number;
  firstName: string;
  email: string;
  region: string;
  createdUtc: string;
}

export interface CreateCustomerRequest {
  firstName: string;
  email: string;
  region: string;
  notes?: string;
}

export interface ValidationErrorResponse {
  errors: string[];
}
