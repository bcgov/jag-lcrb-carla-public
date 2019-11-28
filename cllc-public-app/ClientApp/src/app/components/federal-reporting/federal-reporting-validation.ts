import { ValidatorFn, FormGroup } from '@angular/forms';

const wholeNumberMessage = 'Units must be a whole number less than 10000000.';
const wholeNumberError = [
  { type: 'min', message: wholeNumberMessage },
  { type: 'max', message: wholeNumberMessage },
  { type: 'pattern', message: wholeNumberMessage }
];

const valueMessage = 'Value must be a less than 1000000000 and have 2 decimal places.';
const valueError = [
  { type: 'min', message: valueMessage },
  { type: 'max', message: valueMessage },
  { type: 'pattern', message: valueMessage }
];

const weightMessage = 'Value must be a less than 10000000 and have 3 decimal places.';
const weightError = [
  { type: 'min', message: weightMessage },
  { type: 'max', message: weightMessage },
  { type: 'pattern', message: weightMessage }
];

export const validationErrors = {
  'openingInventory': wholeNumberError,
  'domesticAdditions': wholeNumberError,
  'returnsAdditions': wholeNumberError,
  'otherAdditions': wholeNumberError,
  'domesticReductions': wholeNumberError,
  'returnsReductions': wholeNumberError,
  'destroyedReductions': wholeNumberError,
  'lostReductions': wholeNumberError,
  'otherReductions': wholeNumberError,
  'closingNumber': [
    ...wholeNumberError,
    { type: 'closingNumberMismatch', message: 'Closing Inventory must be equal to (Opening Inventory + Additions - Reductions)' }
  ],
  'closingValue': valueError,
  'closingWeight': weightError,
  'totalSeeds': wholeNumberError,
  'totalSalesToConsumerQty': [
    ...wholeNumberError,
    { type: 'totalSalesToConsumerQty', message: 'Sales quantity must equal domestic reductions' }
  ],
  'totalSalesToConsumerValue': valueError
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
    fg.get('closingNumber').setErrors({ closingNumberMismatch: true });
  } else {
    fg.get('closingNumber').setErrors(null);
  }
  return null;
};

export const SalesValidator: ValidatorFn = (fg: FormGroup) => {
  if (+fg.get('totalSalesToConsumerQty').value !== +fg.get('domesticReductions').value) {
    fg.get('totalSalesToConsumerQty').setErrors({ salesMismatch: true });
  } else {
    fg.get('totalSalesToConsumerQty').setErrors(null);
  }
  return null;
};
