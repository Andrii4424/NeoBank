import { PaymentSystem } from "../../../../enums/payment-system";
import { ICardTariffs } from "./card-tariffs.interface";

export interface ICroppedUserCard{
    id: string;
    userId: string;
    cardTariffId: string;
    cardTariffs: ICardTariffs;
    cardNumber: string;
    chosedPaymentSystem: PaymentSystem;
}