
export interface Gift {
    id: number;
   name: string;
    description: string;
    img: string;
    price: number;
    idDoner: number;
    categoryId?: number;
}

export interface AddGiftDto {
   name: string;
    description: string;
    image?: File;
    price: number;
    idDoner: number;
    categoryId?: number;
}

export interface UpdateGiftDto {
   name: string;
    description: string;
    img: string;
    price: number;
    idDoner: number;
    categoryId?: number;
}
