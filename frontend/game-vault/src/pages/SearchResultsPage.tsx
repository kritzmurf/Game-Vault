import { useState, useEffect, useReducer } from "react"
import { useSearchParams } from "react-router-dom"
import { searchGames } from "../services/api"
import GameCard from "../components/GameCard"
import Pagination from "../components/Pagination" 
import ErrorMessage from "../components/ErrorMessage"
import LoadingThrobber from "../components/LoadingThrobber"
import type { Game } from "../types/game"

const PAGE_SIZE = 20

interface SearchState {
    games: Game[]
    totalCount: number
    loading: boolean
    error: string
}

type SearchAction =
    | { type: "fetchStart" }
    | { type: "fetchSuccess"; games: Game[]; totalCount: number }
    | { type: "fetchError"; error: string }

const initialState: SearchState = {
    games: [],
    totalCount: 0,
    loading: false,
    error: "",
}

function searchReducer(state: SearchState, action: SearchAction): SearchState {
    switch(action.type) {
        case "fetchStart":
            return { ...state, loading: true, error:"" }
        case "fetchSuccess":
            return { ...state, loading: false, games: action.games, totalCount: action.totalCount }
        case "fetchError":
            return { ...state, loading: false, error: action.error }
    }
}

function SearchResultsPage() {
    const [searchParams] = useSearchParams()
    const q = searchParams.get("q") ?? ""
    const [page, setPage] = useState(1)
    const [state, dispatch] = useReducer(searchReducer, initialState)

    useEffect(() => {
        if (!q) return
        dispatch({ type: "fetchStart" })
        searchGames(q, page, PAGE_SIZE)
            .then((data) => {
                dispatch({ type: "fetchSuccess", games: data.items, totalCount: data.totalCount })
            })
            .catch(() => dispatch({ type: "fetchError", error:"Failed to search games" }))
            }, [q, page])

    const totalPages = Math.ceil(state.totalCount / PAGE_SIZE)

    if (state.loading) return <LoadingThrobber />
    if (state.error) return <ErrorMessage message={state.error} onRetry={() => window.location.reload()} />

    return (
        <main className = "max-w-4xl mx-auto px-4 sm:px-6 py-8">
            <h1 className="text-2xl sm:text-3xl font-bold mb-2">Results for "{q}"</h1>
            <p className="text-gray-400 mb-6">{state.totalCount.toLocaleString()} results</p>
            <div className="flex flex-col gap-3">
                {state.games.map((game) => (
                    <GameCard key={game.id} game={game} />
                ))}
            </div>
            <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </main>
    )
}

export default SearchResultsPage
