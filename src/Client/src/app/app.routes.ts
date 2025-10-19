import { Routes } from '@angular/router';
import { Layout } from './common-ui/layout/layout';
import { Home } from './pages/home/home';
import { Login } from './pages/auth/login/login';
import { Register } from './pages/auth/register/register';
import { BankInfo } from './pages/products/bank/bank-info/bank-info';
import { UpdateBank } from './pages/products/bank/update-bank/update-bank';
import { CanActivateAuth } from './auth/access.guard';
import { UsersOwnProfile } from './pages/profile/users-own-profile/users-own-profile';
import { adminGuard } from './auth/admin.guard';
import { CardTariffsInfo } from './pages/cards/tariffs/card-tariffs-info/card-tariffs-info';
import { CardTariffs } from './pages/cards/tariffs/card-tariffs/card-tariffs';
import { UpdateTariffs } from './pages/cards/tariffs/update-tariffs/update-tariffs';
import { AddCardTariffs } from './pages/cards/tariffs/add-card-tariffs/add-card-tariffs';
import { CurrencyRates } from './pages/products/currency-rates/currency-rates';
import { UserCards } from './pages/cards/users/user-cards/user-cards';
import { ChoseCardOptions } from './pages/cards/users/chose-card-options/chose-card-options';
import { UserCardInfo } from './pages/cards/users/user-card-info/user-card-info';
import { Vacancies } from './pages/users/vacancies/vacancies/vacancies';
import { AddVacancy } from './pages/users/vacancies/add-vacancy/add-vacancy';
import { UpdateVacancy } from './pages/users/vacancies/update-vacancy/update-vacancy';

export const routes: Routes = [
    {path: '', component: Layout, children: [
        {path: '', component: Home},
        {path: 'about', component: BankInfo },
        {path: 'about/update', component: UpdateBank, canActivate: [adminGuard]},
        {path: 'my-profile', component: UsersOwnProfile, canActivate: [CanActivateAuth]},
        {path: 'cards', component: CardTariffs},
        {path: 'cards/my-cards', component: UserCards, canActivate: [CanActivateAuth]},
        {path: 'cards/info/:id', component: CardTariffsInfo},
        {path: 'cards/update/:id', component: UpdateTariffs, canActivate: [adminGuard]},
        {path: 'cards/add', component: AddCardTariffs, canActivate: [adminGuard]},
        {path: 'currency-rates', component: CurrencyRates},
        {path: 'cards/my-cards/add/options', component: ChoseCardOptions, canActivate: [CanActivateAuth]},
        {path: 'cards/my-card/:id', component: UserCardInfo, canActivate: [CanActivateAuth]},
        {path: 'vacancies', component: Vacancies},
        {path: 'vacancies/add', component: AddVacancy},
        {path: 'vacancies/update/:id', component: UpdateVacancy},

    ]},
    {path: 'login', component: Login},
    {path: 'signup', component: Register}

];
