export interface GetOrderDto {
    id: number;
    idUser: number;
    win: boolean;
    user?: UserShortDto;
    gift?: OrderShortDto;
}

export interface OrderShortDto {
    id: number;
    name: string;
    price: number;
}

export interface UserShortDto {
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber?: string;
}
