import { createApp } from './app';

const port = Number(process.env['PORT'] ?? 8080);

createApp().listen(port, () => {
  console.log(`portal-frontend-angular-v2 server listening on port ${port}`);
});
