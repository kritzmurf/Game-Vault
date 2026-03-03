import type { Game, PaginatedResponse, Platform } from "../types/game"

const API_PATH = "/api"

export async function getGames(page: number = 1, pageSize: number = 20, platform?: string): Promise<PaginatedResponse<Game>> {
    const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) })
    if (platform) params.append("Platform", platform)
    const response = await fetch(`${API_PATH}/games?${params}`)
    if (!response.ok) throw new Error("Failed to fetch games")
    return response.json()
}

export async function getGameById(id: number) : Promise<Game> {
    const response = await fetch(`${API_PATH}/games/${id}`)
    if (!response.ok) throw new Error("failed to fetch game")
    return response.json()
}

export async function getPlatforms(): Promise<Platform[]> {
    const response = await fetch(`${API_PATH}/games/platforms`)
    if (!response.ok) throw new Error("failed to fetch platforms")
    return response.json()
}

export async function searchGames(q: string, page: number = 1, pageSize: number = 20): Promise<PaginatedResponse<Game>> {
    const params = new URLSearchParams({ q, page: String(page), pageSize: String(pageSize) })
    const response = await fetch(`${API_PATH}/games/search?${params}`)
    if (!response.ok) throw new Error("Failed to search games")
    return response.json()
}
