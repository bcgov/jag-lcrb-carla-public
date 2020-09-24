export function conditionalValidator(predicate, validator) {
    return (formControl => {
        if (!formControl.parent) {
            return null;
        }
        if (predicate()) {
            return validator(formControl);
        }
        return null;
    })
}