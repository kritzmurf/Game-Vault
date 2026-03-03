import { useState, useEffect } from 'react'
import { getPlatforms } from "../services/api"
import PlatformCard from "../components/PlatformCard"
import type { Platform } from "../types/game"

function HomePage() {
    const [platforms, setPlatforms] = useState<Platform[]>([])

    useEffect(() => {
        getPlatforms().then(setPlatforms)
    }, [])

    return (
        <main className="max-w-6xl mx-auto px-6 py-8">
            <h1 className="text-3xl font-bold mb-8">Browse by Platform</h1>
            <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
                {platforms.map((p) => (
                    <PlatformCard key={p.platform} name={p.platform} gameCount={p.game_count} />
                ))}
            </div>
        </main>

    )
}

export default HomePage
