// Customer creation form.
// DEMO NOTE (scenario 1): fields here must stay in step with CreateCustomerRequest on the API.
// Adding a surname means touching this form, the TS types, the C# DTO, the validator,
// the entity and the tests - which is exactly the cross-stack coordination we demonstrate.
import { createCustomer } from '../api.js';
import type { CreateCustomerRequest } from '../types.js';

const REGIONS = ['EMEA', 'AMER', 'APAC'];

export function renderCustomerForm(root: HTMLElement, onCreated: () => void): void {
  root.innerHTML = `
    <form id="customer-form" novalidate>
      <h2>New customer</h2>
      <div class="name-row">
        <label>First name
          <input name="firstName" type="text" required maxlength="50" />
        </label>
        <label>Last name
          <input name="lastName" type="text" required maxlength="50" />
        </label>
      </div>
      <label>Email
        <input name="email" type="email" required />
      </label>
      <label>Region
        <select name="region" required>
          ${REGIONS.map((r) => `<option value="${r}">${r}</option>`).join('')}
        </select>
      </label>
      <button type="submit">Create customer</button>
      <p class="error" id="form-error" role="alert"></p>
    </form>
  `;

  const form = root.querySelector<HTMLFormElement>('#customer-form');
  const errorEl = root.querySelector<HTMLParagraphElement>('#form-error');
  if (!form || !errorEl) return;

  form.addEventListener('submit', async (event) => {
    event.preventDefault();
    errorEl.textContent = '';

    const data = new FormData(form);
    const request: CreateCustomerRequest = {
      firstName: String(data.get('firstName') ?? ''),
      lastName: String(data.get('lastName') ?? ''),
      email: String(data.get('email') ?? ''),
      region: String(data.get('region') ?? '')
    };

    try {
      await createCustomer(request);
      form.reset();
      onCreated();
    } catch (error) {
      errorEl.textContent = error instanceof Error ? error.message : 'Could not create customer.';
    }
  });
}
