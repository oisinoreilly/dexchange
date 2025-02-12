import * as React from 'react';
import * as ReactDOM from 'react-dom';
import 'bootstrap/dist/css/bootstrap.css';
import App from './App.tsx'
import $ from 'jquery';

window.$ = $;

// Workaround for dev, comment for production
const originalFetch = window.fetch;
function replaceUrl(url: string) {
  return url.replace(/^http:\/\/ec2-63-32-159-120\.eu-west-1\.compute\.amazonaws\.com:5001/, "http://localhost:5173/api");
}
function hasExternalUrl(url: string) {
  return url.match(/^http:\/\/ec2-63-32-159-120\.eu-west-1\.compute\.amazonaws\.com:5001/);
}
window.fetch = function customFetch(...args) {
    if (typeof args[0] === "string" && hasExternalUrl(args[0])) {
        args[0] = replaceUrl(args[0]);
    } else if (args[0] && args[0].constructor === Request && hasExternalUrl(args[0].url)){
        args[0] = new Request(
            replaceUrl(args[0].url),
            args[0]
        )
    }
    return originalFetch(...args);
}


// This code starts up the React app when it runs in a browser.
ReactDOM.render(
  <App store={store} />,
  document.getElementById('react-app')
);
