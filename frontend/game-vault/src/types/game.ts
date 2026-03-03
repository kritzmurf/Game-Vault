// Data format for receiving data from C# backend

export interface Game {
    id: number
    title: string
    platform: string
    releaseDate: string | null
    publisher: string | null
    developer: string | null
    description: string | null
    coverArtUrl: string | null
    region: string
}

export interface PaginatedResponse<T> {
    items: T[]
    totalCount: number
    page: number
    pageSize: number
}

export interface Platform {
    platform: string
    game_count: number
}
