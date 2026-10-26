import { PersonalDataConsentDocument } from '@/features/auth/api/auth.api';
import { useId } from 'react';
import { Link } from 'react-router-dom';

type PersonalDataConsentCheckboxProps = {
  checked: boolean;
  onChange: (checked: boolean) => void;
  document: PersonalDataConsentDocument | null;
  error?: string;
};

export default function PersonalDataConsentCheckbox({
  checked,
  onChange,
  document,
  error,
}: PersonalDataConsentCheckboxProps) {
  const id = useId();
  const errorId = `${id}-error`;

  return (
    <div className="space-y-3">
      <label
        htmlFor={id}
        className={`flex cursor-pointer items-start gap-3 rounded-xl border p-3 text-sm leading-6 transition-colors ${
          error
            ? 'border-red-500 bg-red-50 text-red-700 dark:border-red-500 dark:bg-red-950/20 dark:text-red-300'
            : 'border-gray-200 text-gray-800 dark:border-gray-700 dark:text-gray-200'
        }`}
      >
        <input
          id={id}
          type="checkbox"
          checked={checked}
          onChange={(event) => onChange(event.target.checked)}
          disabled={!document}
          aria-invalid={Boolean(error)}
          aria-describedby={error ? errorId : undefined}
          className="mt-1 h-4 w-4 shrink-0 accent-slate-900"
        />

        <span>
          Я даю согласие на обработку персональных данных и подтверждаю, что ознакомился(-ась) с{' '}
          <Link
            to="/privacy"
            target="_blank"
            rel="noopener noreferrer"
            className="underline underline-offset-4 hover:text-slate-950 dark:hover:text-white"
          >
            Политикой обработки персональных данных
          </Link>.
        </span>
      </label>

      {document && (
        <details className="rounded-xl border border-gray-200 dark:border-gray-700">
          <summary className="cursor-pointer px-4 py-3 text-sm font-medium text-primary-700 dark:text-primary-400">
            Прочитать текст согласия
          </summary>

          <div className="max-h-64 overflow-y-auto border-t border-gray-200 px-4 py-3 dark:border-gray-700">
            <pre className="whitespace-pre-wrap break-words font-sans text-sm leading-6 text-gray-700 dark:text-gray-300">
              {document.content}
            </pre>
          </div>
        </details>
      )}

      {!document && !error && (
        <p className="text-sm text-gray-500 dark:text-gray-400">
          Загружаем текст согласия…
        </p>
      )}

      {error && (
        <p id={errorId} role="alert" className="text-sm text-red-600 dark:text-red-400">
          {error}
        </p>
      )}
    </div>
  );
}