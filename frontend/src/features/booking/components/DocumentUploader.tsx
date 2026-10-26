interface Props {
    title: string;
    onChange: (file: File | null) => void;
}

export const DocumentUploader = ({
    title,
    onChange
} : Props) => {
    return (
        <div className="space-y-2">
            <p className="font-medium">
                {title}
            </p>
            <input
                type="file"
                accept="image/*"
                onChange={(e) => onChange(e.target.files?.[0] ?? null)}
            />
        </div>
    );
};