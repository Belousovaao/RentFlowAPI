import { CustomerFormProps, CustomerType } from "@/entities/customer/customer.type";
import { IndividualForm } from "./IndividualForm";
import { EntrepreneurForm } from "./EntrepreneurForm";
import { OrganizationForm } from "./OrganizationForm";

export const CustomerForm = ({
    type,
    customer,
    readOnly = false,
    onCreate,
    onUpdate
}: CustomerFormProps) => {
    switch (type) {
        case CustomerType.Individual:
            return (
                <IndividualForm
                    customer={customer}
                    readOnly={readOnly}
                    onCreate={onCreate}
                    onUpdate={onUpdate}
                />
            );

        case CustomerType.IndividualEntrepreneur:
            return (
                <EntrepreneurForm
                    customer={customer}
                    readOnly={readOnly}
                    onCreate={onCreate}
                    onUpdate={onUpdate}
                />
            );

        case CustomerType.Organization:
            return (
                <OrganizationForm
                    customer={customer}
                    readOnly={readOnly}
                    onCreate={onCreate}
                    onUpdate={onUpdate}
                />
            );

        default:
            return null;
    }
};