// Read-only table of existing customers.
import { listCustomers } from '../api.js';

export async function renderCustomerList(root: HTMLElement): Promise<void> {
  try {
    const customers = await listCustomers();

    if (customers.length === 0) {
      root.innerHTML = '<p>No customers yet.</p>';
      return;
    }

    root.innerHTML = `
      <table>
        <thead>
          <tr><th>ID</th><th>Name</th><th>Email</th><th>Region</th></tr>
        </thead>
        <tbody>
          ${customers
            .map(
              (c) => `<tr>
                <td>${c.id}</td>
                <td>${c.firstName} ${c.lastName}</td>
                <td>${c.email}</td>
                <td>${c.region}</td>
              </tr>`
            )
            .join('')}
        </tbody>
      </table>
    `;
  } catch (error) {
    root.innerHTML = `<p class="error">${
      error instanceof Error ? error.message : 'Could not load customers.'
    }</p>`;
  }
}
