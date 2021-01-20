import { ValidatorFn, FormGroup } from "@angular/forms";

const wholeNumberMessage = "Units must be a whole number less than 10,000,000.";
const wholeNumberError = [
  { type: "min", message: wholeNumberMessage },
  { type: "max", message: wholeNumberMessage },
  { type: "pattern", message: wholeNumberMessage }
];

const valueMessage = "Value must be less than $1,000,000,000 and have 2 decimal places.";
const valueError = [
  { type: "min", message: valueMessage },
  { type: "max", message: valueMessage },
  { type: "pattern", message: valueMessage }
];

const weightMessage = "Weight is required and must be less than 1,000kg and have 3 decimal places.";
const weightError = [
  { type: "required", message: weightMessage },
  { type: "min", message: weightMessage },
  { type: "max", message: weightMessage },
  { type: "pattern", message: weightMessage }
];

const descriptionMessage = "Description is required for this product type.";
const descriptionError = [
  { type: "required", message: descriptionMessage }
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
  'totalSalesToConsumerValue': valueError,
  'otherDescription': descriptionError
};

export const formValidationErrors = {
  'closingNumberMismatch': "Closing Inventory must be equal to (Opening Inventory + Additions - Reductions)",
  'salesMismatch': "Sales quantity must equal domestic reductions",
  'salesNumberNonZero': "Sales quantity must be greater than 1",
  'salesValueNonZero': "Sales value must be greater than 1",
  'closingNumberNonZero': "Closing quantity must be greater than 1",
  'closingWeightNonZero': "Closing weight must be greater than 0.001",
  'closingValueNonZero': "Closing value must be greater than 1",
  'closingSeedsNonZero': "Closing seeds must be greater than 1"
};

export const ClosingInventoryValidator: ValidatorFn = (fg: FormGroup) => {
  const validation = {};
  const closingWeight = +fg.get("closingWeight").value;
  const closingNumber = +fg.get("closingNumber").value;
  const closingValue = +fg.get("closingValue").value;
  const additions = [
    +fg.get("openingInventory").value,
    +fg.get("domesticAdditions").value,
    +fg.get("returnsAdditions").value,
    +fg.get("otherAdditions").value
  ];
  const reductions = [
    +fg.get("domesticReductions").value,
    +fg.get("returnsReductions").value,
    +fg.get("destroyedReductions").value,
    +fg.get("lostReductions").value,
    +fg.get("otherReductions").value
  ];

  const total = additions.reduce((n, curr) => n + curr, 0) - reductions.reduce((n, curr) => n + curr, 0);
  if (total !== +fg.get("closingNumber").value) {
    validation["closingNumberMismatch"] = true;
  }
  if ((closingWeight !== 0 || closingValue !== 0) && closingNumber < 1) {
    validation["closingNumberNonZero"] = true;
  }
  if (Object.keys(validation).length > 0) {
    return validation;
  }
  return null;
};

export const ClosingValueValidator: ValidatorFn = (fg: FormGroup) => {
  const closingWeight = +fg.get("closingWeight").value;
  const closingNumber = +fg.get("closingNumber").value;
  const closingValue = +fg.get("closingValue").value;
  if ((closingWeight === 0 && closingNumber === 0) || closingValue >= 1) {
    return null;
  }
  return { 'closingValueNonZero': true };
};

export const ClosingWeightValidator: ValidatorFn = (fg: FormGroup) => {
  const closingWeight = +fg.get("closingWeight").value;
  const closingNumber = +fg.get("closingNumber").value;
  const closingValue = +fg.get("closingValue").value;
  if ((closingValue === 0 && closingNumber === 0) || closingWeight >= 0.001) {
    return null;
  }
  return { 'closingWeightNonZero': true };
};

export const ClosingSeedsTotalValidator: ValidatorFn = (fg: FormGroup) => {
  const totalSeeds = +fg.get("totalSeeds").value;
  const closingNumber = +fg.get("closingNumber").value;
  const closingValue = +fg.get("closingValue").value;
  if ((closingValue === 0 && closingNumber === 0) || totalSeeds >= 1) {
    return null;
  }
  return { 'closingSeedsNonZero': true };
};

export const SalesValidator: ValidatorFn = (fg: FormGroup) => {
  const validation = {};
  const totalSalesToConsumerQty = +fg.get("totalSalesToConsumerQty").value;
  const totalSalesToConsumerValue = +fg.get("totalSalesToConsumerValue").value;
  if (totalSalesToConsumerQty > 0 && totalSalesToConsumerValue < 1) {
    validation["salesValueNonZero"] = true;
  }
  if (totalSalesToConsumerValue > 0 && totalSalesToConsumerQty < 1) {
    validation["salesNumberNonZero"] = true;
  }
  if (totalSalesToConsumerQty !== +fg.get("domesticReductions").value) {
    validation["salesMismatch"] = true;
  }
  if (Object.keys(validation).length > 0) {
    return validation;
  }
  return null;
};
