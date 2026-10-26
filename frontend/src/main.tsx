import React from "react";
import ReactDOM from "react-dom/client";
import { QueryProvider } from './app/providers/QueryProvider';
import { I18nProvider } from './app/providers/I18nProvider';
import { Toaster } from "react-hot-toast";
import App  from "./App";
import "./index.css";

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <QueryProvider>
        <I18nProvider>
          <App />
          <Toaster
            position="top-right"
            toastOptions={{
              duration: 4000,
              style: {
                borderRadius: '12px',
                background: "#fff",
                color: "#111827"
              },
            }}
          />
        </I18nProvider>
    </QueryProvider>
  </React.StrictMode>
);
