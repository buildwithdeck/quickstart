import { createRoot } from "react-dom/client"
import { useDeckLink } from "react-deck-link"
import { useEffect, useRef, useState } from "react"

// Prefix event with timestamp, eg. [14:05:01] eventName
const logEvent = (event: string) => () => {
  const timestamp = new Date().toLocaleTimeString()
  document.getElementById("js-events")!.innerHTML += `[${timestamp}] ${event}<br />`
}

const OpenLink = ({ token = "" }) => {
  const { open, isReady } = useDeckLink({
    token, 
    onExit: logEvent("onExit()"),
    onSuccess: logEvent("onSuccess()") ,
    source_id: "a47c74e8-ae4a-425e-8583-922d5515654a",
  })

  useEffect(() => {
    isReady && open()
  }, [isReady, open])

  return (
    <button onClick={open} disabled={!isReady}>
      Connect
    </button>
  )
}

const LinkForm = () => {
  const [confirmed, setConfirmed] = useState(false)
  const tokenRef = useRef<HTMLInputElement>(null)

  return (
    <div>
      <input
        type="text"
        placeholder="link-00000000-0000-0000-0000-000000000000"
        ref={tokenRef}
        disabled={confirmed}
      />
      {!confirmed && (
        <button onClick={() => setConfirmed(true)}>Connect</button>
      )}
      {confirmed && <OpenLink token={tokenRef.current.value} />}
    </div>
  )
}

createRoot(document.getElementById("app")!).render(<LinkForm />)
