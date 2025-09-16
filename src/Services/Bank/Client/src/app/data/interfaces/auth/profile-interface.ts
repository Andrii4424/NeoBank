export interface IProfile{
    id: string;
    email: string | null;
    firstName: string| null;
    surname: string | null;
    patronymic: string | null;
    dateOfBirth: string | null;
    taxId: string | null;
    avatarPath: string | null;
    role: string | null;
    isVerified: boolean | null;
    phoneNumber: string | null;
}


