export interface IRecoveryPassword{
    email: string | null;
    refreshCode: string | null;
    newPassword: string | null;
}