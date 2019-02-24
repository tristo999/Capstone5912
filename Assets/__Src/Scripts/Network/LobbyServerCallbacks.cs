using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "Lobby")]
class LobbyServerCallbacks : Bolt.GlobalEventListener
{
    public override void Connected(BoltConnection connection) {
    }

    public override void Disconnected(BoltConnection connection) {
        LobbyNetworkedManager.Instance.RemovePlayersFromConnection(connection);
    }
}
