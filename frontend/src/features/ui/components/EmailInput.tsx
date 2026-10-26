import {
    type FocusEvent,
    type InputHTMLAttributes,
    type KeyboardEvent,
    useMemo,
    useRef,
    useState,
} from 'react';

interface EmailInputProps
    extends Omit<
        InputHTMLAttributes<HTMLInputElement>,
        'value' | 'onChange'
    > {
    value: string;
    onChange: (value: string) => void;
}

const COMMON_DOMAINS = [
    'gmail.com',
    'mail.ru',
    'yandex.ru',
    'yahoo.com',
    'outlook.com',
    'icloud.com',
    'rambler.ru',
    'bk.ru',
    'inbox.ru',
    'list.ru',
];

const MAX_SUGGESTIONS = 4;
const MAX_TYPO_DISTANCE = 2;

function getDomain(value: string): string | null {
    const atIndex = value.lastIndexOf('@');

    if (atIndex === -1) {
        return null;
    }

    return value.slice(atIndex + 1).toLowerCase();
}

function replaceDomain(
    value: string,
    domain: string
): string {
    const atIndex = value.lastIndexOf('@');

    if (atIndex === -1) {
        return value;
    }

    return `${value.slice(0, atIndex + 1)}${domain}`;
}

function levenshteinDistance(
    first: string,
    second: string
): number {
    const rows = second.length + 1;
    const columns = first.length + 1;

    const matrix = Array.from(
        { length: rows },
        () => Array<number>(columns).fill(0)
    );

    for (let row = 0; row < rows; row++) {
        matrix[row][0] = row;
    }

    for (let column = 0; column < columns; column++) {
        matrix[0][column] = column;
    }

    for (let row = 1; row < rows; row++) {
        for (let column = 1; column < columns; column++) {
            const cost =
                second[row - 1] === first[column - 1]
                    ? 0
                    : 1;

            matrix[row][column] = Math.min(
                matrix[row - 1][column] + 1,
                matrix[row][column - 1] + 1,
                matrix[row - 1][column - 1] + cost
            );
        }
    }

    return matrix[rows - 1][columns - 1];
}

function getSuggestions(
    domain: string
): string[] {
    if (!domain) {
        return COMMON_DOMAINS.slice(
            0,
            MAX_SUGGESTIONS
        );
    }

    return COMMON_DOMAINS
        .filter((item) =>
            item.startsWith(domain)
        )
        .slice(0, MAX_SUGGESTIONS);
}

function getTypoSuggestion(
    domain: string
): string | null {
    if (!domain || domain.length < 4) {
        return null;
    }

    let bestMatch: string | null = null;
    let bestDistance = Infinity;

    for (const candidate of COMMON_DOMAINS) {
        const distance = levenshteinDistance(
            domain,
            candidate
        );

        if (
            distance <= MAX_TYPO_DISTANCE &&
            distance < bestDistance
        ) {
            bestDistance = distance;
            bestMatch = candidate;
        }
    }

    return bestMatch;
}

export const EmailInput = ({
    value,
    onChange,
    onBlur,
    onKeyDown,
    ...props
}: EmailInputProps) => {
    const inputRef =
        useRef<HTMLInputElement>(null);

    const [isFocused, setIsFocused] =
        useState(false);

    const domain = useMemo(
        () => getDomain(value),
        [value]
    );

    const suggestions = useMemo(() => {
        if (!isFocused || !domain) {
            return [];
        }

        return getSuggestions(domain);
    }, [domain, isFocused]);

    const typoSuggestion = useMemo(() => {
        if (!isFocused || !domain) {
            return null;
        }

        const suggestion =
            getTypoSuggestion(domain);

        if (
            !suggestion ||
            suggestion === domain
        ) {
            return null;
        }

        return suggestion;
    }, [domain, isFocused]);

    const hasAtSymbol = value.includes('@');

    const selectDomain = (selectedDomain: string) => {
        const nextValue = replaceDomain(
            value,
            selectedDomain
        );

        onChange(nextValue);
    };

    const handleKeyDown = ( event: KeyboardEvent<HTMLInputElement>) => {
        if (event.key === 'Enter' && typoSuggestion) {
            event.preventDefault();
            selectDomain(typoSuggestion);
            return;
        }

        onKeyDown?.(event);
    };

    const handleBlur = (event: FocusEvent<HTMLInputElement>) => {
        /*
         * Даём клику по подсказке отработать
         * до закрытия списка.
         */
        window.setTimeout(() => {
            setIsFocused(false);
        }, 100);

        onBlur?.(event);
    };

    return (
        <div className="relative">
            <input
                {...props}
                ref={inputRef}
                type="email"
                inputMode="email"
                autoComplete="email"
                value={value}
                onChange={(event) =>
                    onChange(event.target.value)
                }
                onFocus={() =>
                    setIsFocused(true)
                }
                onBlur={handleBlur}
                onKeyDown={handleKeyDown}
            />

            {isFocused &&
                hasAtSymbol &&
                suggestions.length > 0 && (
                    <div className="absolute left-0 right-0 top-full z-20 mt-1 overflow-hidden rounded-xl border border-gray-200 bg-white shadow-lg dark:border-gray-700 dark:bg-gray-900">
                        {suggestions.map(
                            (suggestion) => (
                                <button
                                    key={suggestion}
                                    type="button"
                                    className="block w-full px-4 py-2.5 text-left text-sm text-gray-700 hover:bg-gray-50 dark:text-gray-200 dark:hover:bg-gray-800"
                                    onMouseDown={(event) =>
                                        event.preventDefault()
                                    }
                                    onClick={() =>
                                        selectDomain(
                                            suggestion
                                        )
                                    }
                                >
                                    {value.slice(
                                        0,
                                        value.lastIndexOf(
                                            '@'
                                        ) + 1
                                    )}
                                    <span className="font-medium">
                                        {suggestion}
                                    </span>
                                </button>
                            )
                        )}
                    </div>
                )}

            {isFocused &&
                hasAtSymbol &&
                typoSuggestion && (
                    <div className="mt-2 flex items-center justify-between gap-3 rounded-xl bg-amber-50 px-3 py-2.5 text-sm text-amber-800 dark:bg-amber-950/30 dark:text-amber-200">
                        <span>
                            Возможно, вы имели в виду{' '}
                            <button
                                type="button"
                                className="font-semibold underline underline-offset-2"
                                onMouseDown={(event) =>
                                    event.preventDefault()
                                }
                                onClick={() =>
                                    selectDomain(
                                        typoSuggestion
                                    )
                                }
                            >
                                {replaceDomain(
                                    value,
                                    typoSuggestion
                                )}
                            </button>
                            ?
                        </span>
                    </div>
                )}
        </div>
    );
};