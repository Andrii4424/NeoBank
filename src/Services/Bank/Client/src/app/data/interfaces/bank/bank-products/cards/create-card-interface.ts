import { Currency } from "../../../../enums/currency";
import { PaymentSystem } from "../../../../enums/payment-system";

export interface ICreateCard{
    cardTariffId : string;
    chosenCurrency : Currency;
    chosenPaymentSystem: PaymentSystem;
    Pin: string;
}

