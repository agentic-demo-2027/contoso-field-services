// SPA entry point for the Contoso Field Services customer registry.
import { renderCustomerForm } from './components/CustomerForm.js';
import { renderCustomerList } from './components/CustomerList.js';

const formRoot = document.querySelector<HTMLElement>('#form-root');
const listRoot = document.querySelector<HTMLElement>('#list-root');

if (formRoot && listRoot) {
  const refresh = () => void renderCustomerList(listRoot);
  renderCustomerForm(formRoot, refresh);
  refresh();
}
