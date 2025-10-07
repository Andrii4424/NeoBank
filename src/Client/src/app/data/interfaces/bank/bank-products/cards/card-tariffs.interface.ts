import { CardLevel } from "../../../../enums/card-level";
import { CardType } from "../../../../enums/card-type";
import { Currency } from "../../../../enums/currency";
import { PaymentSystem } from "../../../../enums/payment-system";

export interface ICardTariffs{
    id: string | null;
    bankId: string | null;
    cardName: string | null;
    type: CardType | null;
    level: CardLevel | null;
    validityPeriod: number | null;
    maxCreditLimit: number| null;
    enabledPaymentSystems: PaymentSystem[] | null;
    interestRate: number | null;
    enableCurrency: Currency[] | null;
    annualMaintenanceCost: number | null;
    p2PInternalCommission: number | null;
    bin: string | null;
    cardColor: string | null;
}
