// Thin HTTP client for the customer API.
import type { Customer, CreateCustomerRequest } from './types.js';

const BASE_URL = 'http://localhost:5266';

export async function listCustomers(): Promise<Customer[]> {
  const response = await fetch(`${BASE_URL}/api/customers`);
  if (!response.ok) throw new Error(`Failed to load customers (${response.status})`);
  return response.json() as Promise<Customer[]>;
}

export async function createCustomer(request: CreateCustomerRequest): Promise<Customer> {
  const response = await fetch(`${BASE_URL}/api/customers`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request)
  });

  if (!response.ok) {
    const body = await response.json().catch(() => ({ errors: ['Unexpected error'] }));
    throw new Error(Array.isArray(body.errors) ? body.errors.join(', ') : body.message);
  }

  return response.json() as Promise<Customer>;
}
