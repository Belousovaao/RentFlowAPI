import api from "@/shared/api/axios"

export const verificationApi = {
    upload: async (files: {
        passportMain: File;
        passportRegistration: File;
        driveFront: File;
        driveBack: File;
    }) => {
        const formData = new FormData();
        formData.append("passportMain", files.passportMain);
        formData.append("passportRegistration", files.passportRegistration);
        formData.append("driveFront", files.driveFront);
        formData.append("driveBack", files.driveBack);

        await api.post("/customer/documents", formData);
    }
};