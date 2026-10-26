import { useI18n } from "@/app/providers/I18nProvider";
import { verificationApi } from "@/entities/booking/verification.api";
import { DocumentUploader } from "@/features/booking/components/DocumentUploader";
import { Button } from "@/features/ui/components/Button";
import { useState } from "react"

export const VerificationPage = () => {
    const { t } = useI18n();
    const [passportMain, setPassportMain] = useState<File | null>(null);
    const [passportRegistration, setPassportRegistration] = useState<File | null>(null);

    const [driveFront, setDriveFront] = useState<File | null>(null);
    const [driveBack, setDriveBack] = useState<File | null>(null);

    const [loading, setLoading] = useState(false);
    const handleSubmit = async () => {
        if (!passportMain || !passportRegistration || !driveFront || !driveBack)
            return;

        setLoading(true);
        try
        {
            await verificationApi.upload({passportMain, passportRegistration, driveFront, driveBack});
        }
        finally
        {
            setLoading(false);
        }
    };

    return (
        <div className="max-w-2xl mx-auto py-12 space-y-8">
            <h1 className="text-3xl font-bold">
                {t('verification.head')}
            </h1>

            <p className="text-gray-500">
                {t('verification.info')}
            </p>
            <DocumentUploader
                title={t('verification.passport.main')}
                onChange={setPassportMain}
            />
            <DocumentUploader
                title={t('verification.passport.registration')}
                onChange={setPassportRegistration}
            />
            <DocumentUploader
                title={t('verification.drive.front')}
                onChange={setDriveFront}
            />
            <DocumentUploader
                title={t('verification.drive.back')}
                onChange={setDriveBack}
            />

            <Button
                onClick={handleSubmit}
                disabled={loading}
            >
                {t('verification.send')}
            </Button>
        </div>
    )

}