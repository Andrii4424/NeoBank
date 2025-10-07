import { HttpContextToken } from "@angular/common/http";

export const SKIP_LOGIN_REDIRECT = new HttpContextToken<boolean>(() => false);
