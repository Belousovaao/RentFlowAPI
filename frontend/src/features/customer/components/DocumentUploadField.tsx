import { useRef, useState } from "react";
import { CheckCircle2, FileText, Image as ImageIcon, UploadCloud, X } from "lucide-react";

interface Props {
    title: string;
    file: File | null;
    onChange: (file: File | null) => void;
}

const MAX_FILE_SIZE = 10 * 1024 * 1024;

const ACCEPTED_TYPES = [
    "image/jpeg",
    "image/png",
    "application/pdf",
];

export const DocumentUploadField = ({
    title,
    file,
    onChange,
}: Props) => {
    const inputRef = useRef<HTMLInputElement>(null);
    const [isDragging, setIsDragging] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const validateFile = (selectedFile: File): boolean => {
        if (!ACCEPTED_TYPES.includes(selectedFile.type)) {
            setError("Можно загрузить JPG, PNG или PDF");
            return false;
        }

        if (selectedFile.size > MAX_FILE_SIZE) {
            setError("Размер файла не должен превышать 10 МБ");
            return false;
        }

        setError(null);
        return true;
    };

    const handleFile = (selectedFile: File | undefined) => {
        if (!selectedFile) {
            return;
        }

        if (validateFile(selectedFile)) {
            onChange(selectedFile);
        }
    };

    const handleInputChange = (
        event: React.ChangeEvent<HTMLInputElement>
    ) => {
        handleFile(event.target.files?.[0]);

        // Позволяет повторно выбрать тот же самый файл
        event.target.value = "";
    };

    const handleDrop = (event: React.DragEvent<HTMLDivElement>) => {
        event.preventDefault();
        setIsDragging(false);

        handleFile(event.dataTransfer.files?.[0]);
    };

    const isPdf = file?.type === "application/pdf";

    return (
        <div className="space-y-2">
            <div className="flex items-center justify-between">
                <p className="text-sm font-medium text-gray-800 dark:text-gray-200">
                    {title}
                </p>

                {file && (
                    <span className="flex items-center gap-1 text-xs text-green-600">
                        <CheckCircle2 className="w-4 h-4" />
                        Загружен
                    </span>
                )}
            </div>

            {!file ? (
                <div
                    onDragOver={(event) => {
                        event.preventDefault();
                        setIsDragging(true);
                    }}
                    onDragLeave={() => setIsDragging(false)}
                    onDrop={handleDrop}
                    onClick={() => inputRef.current?.click()}
                    className={`relative flex flex-col items-center justify-center min-h-36 px-6 py-6 rounded-xl border-2border-dashed transition-all
                        ${
                            isDragging
                                ? "border-primary-500 bg-primary-50 dark:bg-primary-950/30"
                                : "border-gray-300 hover:border-primary-400 hover:bg-gray-50 dark:border-gray-700 dark:hover:bg-gray-800/50"
                        }
                    `}
                >
                    <UploadCloud
                        className={`w-9 h-9 mb-3
                            ${
                                isDragging
                                    ? "text-primary-600"
                                    : "text-gray-400"
                            }
                        `}
                    />

                    <p className="text-sm font-medium text-gray-700 dark:text-gray-200">
                        Перетащите документ сюда
                    </p>

                    <p className="text-xs text-gray-400 my-1">
                        или
                    </p>

                    <button
                        type="button"
                        onClick={(event) => {
                            event.stopPropagation();
                            inputRef.current?.click();
                        }}
                        className="text-sm font-medium text-primary-600 hover:text-primary-700"
                    >
                        Выбрать файл
                    </button>

                    <p className="mt-3 text-xs text-gray-400">
                        JPG, PNG или PDF · до 10 МБ
                    </p>

                    <input
                        ref={inputRef}
                        type="file"
                        accept=".jpg,.jpeg,.png,.pdf,image/jpeg,image/png,application/pdf"
                        className="hidden"
                        onChange={handleInputChange}
                    />
                </div>
            ) : (
                <div className="rounded-xl border border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-800/50 p-3">
                    <div className="flex items-center gap-3">
                        {isPdf ? (
                            <div className="flex h-14 w-14 shrink-0 items-center justify-center rounded-lg bg-red-50 text-red-500 dark:bg-red-950/30">
                                <FileText className="w-7 h-7" />
                            </div>
                        ) : (
                            <div className="flex h-14 w-14 shrink-0 items-center justify-center overflow-hidden rounded-lg bg-gray-100 dark:bg-gray-700">
                                <img
                                    src={URL.createObjectURL(file)}
                                    alt={file.name}
                                    className="h-full w-full object-cover"
                                />
                            </div>
                        )}

                        <div className="min-w-0 flex-1">
                            <div className="flex items-center gap-2">
                                {!isPdf && (
                                    <ImageIcon className="w-4 h-4 text-gray-400 shrink-0" />
                                )}

                                {isPdf && (
                                    <FileText className="w-4 h-4 text-red-500 shrink-0" />
                                )}

                                <p className="truncate text-sm font-medium text-gray-800 dark:text-gray-200">
                                    {file.name}
                                </p>
                            </div>

                            <p className="mt-1 text-xs text-gray-400">
                                {isPdf ? "PDF" : "Изображение"} ·{" "}
                                {(file.size / 1024 / 1024).toFixed(1)} МБ
                            </p>
                        </div>

                        <div className="flex items-center gap-1">
                            <button
                                type="button"
                                onClick={() => inputRef.current?.click()}
                                className="rounded-lg px-3 py-2 text-xs font-medium text-primary-600 transition hover:bg-primary-50 dark:hover:bg-primary-950/30"
                            >
                                Заменить
                            </button>

                            <button
                                type="button"
                                onClick={() => onChange(null)}
                                className="rounded-lg p-2 text-gray-400 transition hover:bg-gray-200 hover:text-gray-600 dark:hover:bg-gray-700"
                                aria-label="Удалить файл"
                            >
                                <X className="w-4 h-4" />
                            </button>
                        </div>

                        <input
                            ref={inputRef}
                            type="file"
                            accept=".jpg,.jpeg,.png,.pdf,image/jpeg,image/png,application/pdf"
                            className="hidden"
                            onChange={handleInputChange}
                        />
                    </div>
                </div>
            )}

            {error && (
                <p className="text-xs text-red-500">
                    {error}
                </p>
            )}
        </div>
    );
};
