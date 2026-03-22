import { useState, useEffect, useReducer } from "react"
import { useParams } from "react-router-dom"
import { getGames } from "../services/api"
import GameCard from "../components/GameCard"
import Pagination from "../components/Pagination"
import LoadingThrobber from "../components/LoadingThrobber"
import ErrorMessage from "../components/ErrorMessage"
import type { Game } from "../types/game"
import { formatPlatformName } from "../utils/formatPlatformName"

const PAGE_SIZE = 20

interface PlatformState {
    games: Game[]
    totalCount: number
    loading: boolean
    error: string
}

type PlatformAction =
    | { type: "fetchStart" }
    | { type: "fetchSuccess"; games: Game[]; totalCount: number }
    | { type: "fetchError"; error: string }

const initialState: PlatformState = {
    games: [],
    totalCount: 0,
    loading: true,
    error: "",
}

function platformReducer(state: PlatformState, action: PlatformAction): PlatformState {
    switch (action.type) {
        case "fetchStart":
            return { ...state, loading: true, error: "" }
        case "fetchSuccess":
            return { ...state, loading: false, games: action.games, totalCount: action.totalCount }
        case "fetchError":
            return { ...state, loading: false, error: action.error }
    }
}

function PlatformPage() {
    const { name } = useParams()
    const [state, dispatch] = useReducer(platformReducer, initialState)
    const [page, setPage] = useState(1)
 
    useEffect(() => {
        if (!name) return
        dispatch({ type: "fetchStart"})

        //populate with paginated response values
        getGames(page, PAGE_SIZE, name)
            .then((data) => {
                dispatch({ type: "fetchSuccess", games: data.items, totalCount: data.totalCount })
            })
            .catch(() =>  dispatch({ type: "fetchError", error: "Failed to load games" }))
    },[name, page])

    const totalPages = Math.ceil(state.totalCount / PAGE_SIZE)

    if (state.loading) return <LoadingThrobber />
    if (state.error) return <ErrorMessage message={state.error} onRetry={() => window.location.reload()} />

    return (
        <main className="max-w-4xl mx-auto px-4 sm:px-6 py-8">
            <h1 className="text-2xl sm:text-3xl font-bold mb-2">{formatPlatformName(name ?? "")}</h1>
            <p className="text-gray-400 mb-6">{state.totalCount.toLocaleString()} games</p>
            <div className="flex flex-col gap-3">
                {state.games.map((game) => (
                    <GameCard key={game.id} game={game} />
                ))}
            </div>
            <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </main>
    )
}

export default PlatformPage
