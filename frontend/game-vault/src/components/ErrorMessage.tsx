interface ErrorMessageProps{
    message: string
    onRetry?: () => void
}

function ErrorMessage({ message, onRetry }: ErrorMessageProps) {
    return (
        <main className="max-w-6xl mx-auto px-6 py-8">
            <div className="flex flex-col items-center justify-center py-20 text-center">
                <p className="text-red-400 text-lg mb-4">{message}</p>
                {onRetry && (
                    <button
                        className="px-4 py-2 bg-gray-800 rounded-lg hover:bg-gray-700 transition-colors"
                        onClick={onRetry}
                    >
                        Try Again
                    </button>
                )}
            </div>
        </main>
    )
}

export default ErrorMessage
