import { CardStatus } from "../../../../enums/card-status";
import { Currency } from "../../../../enums/currency";
import { PaymentSystem } from "../../../../enums/payment-system";
import { ICardTariffs } from "./card-tariffs.interface";

export interface IUserCards{
    id : string;
    userId: string;
    cardTariffId: string;
    cardTariffs: ICardTariffs;
    cardNumber : string;
    expiryDate : string;
    chosedPaymentSystem : PaymentSystem;
    chosenCurrency : Currency;
    pin : string;
    status : CardStatus;
    cvv : string;
    balance : number;
    CreditLimit: number;
}