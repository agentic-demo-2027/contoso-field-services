// Shared shapes between the SPA and the customer API.

export interface Customer {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  region: string;
  createdUtc: string;
}

export interface CreateCustomerRequest {
  firstName: string;
  lastName: string;
  email: string;
  region: string;
}

export interface ValidationErrorResponse {
  errors: string[];
}
