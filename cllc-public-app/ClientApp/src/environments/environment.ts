// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: false,
  lite: false,
  development: true,
};

// TODO: Remove this when proper configuration is in place
export const generatedText = {
  verificationBadge: `<a href="#" onclick="window.open('https://orgbook-app-b7aa30-dev.apps.silver.devops.gov.bc.ca/verify/BC123456', '_blank', 'width=800,height=600'); return false;">Validate</a>`
};