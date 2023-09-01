export type PagedResult<T> =
    {
        results: T[]
        pageCount: number
        totalCount: number
    }


export type Auction = {
    reservePrice: number,
    soldAmount: number,
    currentHighBid: number,
    createdAt: string
    updatedAt: string,
    auctionEnd: string,
    seller: string,
    winner?: string,
    make: string,
    model: string,
    year: number,
    color: string,
    mileage: number,
    status: string,
    imageUrl: string,
    id: string
}