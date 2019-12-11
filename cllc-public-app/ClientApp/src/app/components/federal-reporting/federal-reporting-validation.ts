import { ValidatorFn, FormGroup } from '@angular/forms';

const wholeNumberMessage = 'Units must be a whole number less than 10,000,000.';
const wholeNumberError = [
  { type: 'min', message: wholeNumberMessage },
  { type: 'max', message: wholeNumberMessage },
  { type: 'pattern', message: wholeNumberMessage }
];

const valueMessage = 'Value must be less than $1,000,000,000 and have 2 decimal places.';
const valueError = [
  { type: 'min', message: valueMessage },
  { type: 'max', message: valueMessage },
  { type: 'pattern', message: valueMessage }
];

const weightMessage = 'Weight must be less than 1,000kg and have 3 decimal places.';
const weightError = [
  { type: 'min', message: weightMessage },
  { type: 'max', message: weightMessage },
  { type: 'pattern', message: weightMessage }
];

export const fieldValidationErrors = {
  'openingInventory': wholeNumberError,
  'domesticAdditions': wholeNumberError,
  'returnsAdditions': wholeNumberError,
  'otherAdditions': wholeNumberError,
  'domesticReductions': wholeNumberError,
  'returnsReductions': wholeNumberError,
  'destroyedReductions': wholeNumberError,
  'lostReductions': wholeNumberError,
  'otherReductions': wholeNumberError,
  'closingNumber': wholeNumberError,
  'closingValue': valueError,
  'closingWeight': weightError,
  'totalSeeds': wholeNumberError,
  'totalSalesToConsumerQty': wholeNumberError,
  'totalSalesToConsumerValue': valueError
};

export const formValidationErrors = {
  'closingNumberMismatch': 'Closing Inventory must be equal to (Opening Inventory + Additions - Reductions)',
  'salesMismatch': 'Sales quantity must equal domestic reductions'
};

export const ClosingInventoryValidator: ValidatorFn = (fg: FormGroup) => {
  const additions = [
    +fg.get('openingInventory').value,
    +fg.get('domesticAdditions').value,
    +fg.get('returnsAdditions').value,
    +fg.get('otherAdditions').value
  ];
  const reductions = [
    +fg.get('domesticReductions').value,
    +fg.get('returnsReductions').value,
    +fg.get('destroyedReductions').value,
    +fg.get('lostReductions').value,
    +fg.get('otherReductions').value
  ];

  const total = additions.reduce((n, curr) => n + curr, 0) - reductions.reduce((n, curr) => n + curr, 0);
  if (total !== +fg.get('closingNumber').value) {
    return { closingNumberMismatch: true };
  }
  return null;
};

export const SalesValidator: ValidatorFn = (fg: FormGroup) => {
  if (+fg.get('totalSalesToConsumerQty').value !== +fg.get('domesticReductions').value) {
    return { salesMismatch: true };
  }
  return null;
};
