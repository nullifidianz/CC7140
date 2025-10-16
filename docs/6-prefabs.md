#### 🧱 **Prefab: Player**

**Descrição:**
Personagem principal controlado pelo jogador.

**Quando é utilizado:**
Presente em todas as fases, sendo o centro da interação do jogo.

**Componentes:**

* **Sprite:** animações de idle, walk, jump e death.
* **Colisores:**

  * `BoxCollider2D` (corpo principal).
  * `FeetCollider` (para detectar contato com o chão).
* **Fonte de Áudio:** pulo, dano, coleta de chave.
* **Scripts:**

  * `PlayerMovement.cs`
  * `PlayerDeathHandler.cs`
  * `PlayerInteraction.cs`

**Comportamento dos scripts:**

* **`PlayerMovement`**

  * Gerencia input horizontal (`A/D`, `←/→`) e pulo (`Space`).
  * Aplica força no `Rigidbody2D` para movimentar.
  * Alterna estados e animações conforme ação.
  * Impede pulo múltiplo sem estar no solo.

* **`PlayerDeathHandler`**

  * Detecta colisão com armadilhas.
  * Reproduz som e animação de morte.
  * Reinicia a fase via `SceneManager.LoadScene()`.

* **`PlayerInteraction`**

  * Detecta colisão com `Key` e `Artifact`.
  * Atualiza contador global de chaves.
  * Libera ativação do artefato ao atingir todas as chaves.

---

#### 🗝️ **Prefab: Key (Chave)**

**Descrição:**
Item coletável necessário para completar a fase.

**Quando é utilizado:**
Distribuído ao longo da fase em locais de difícil acesso.

**Componentes:**

* **Sprite:** chave dourada com brilho pulsante.
* **Colisor:** `CircleCollider2D` (isTrigger).
* **Fonte de Áudio:** som curto de coleta.
* **Scripts:**

  * `KeyPickup.cs`

**Comportamento dos scripts:**

* **`KeyPickup`**

  * Detecta colisão com o jogador.
  * Incrementa contador global (`GameManager.AddKey()`).
  * Emite som de coleta.
  * Destroi o objeto após ser coletado.

---

#### ⚙️ **Prefab: Artefato (Final da Fase)**

**Descrição:**
Artefato misterioso que permite sair do mundo bugado quando todas as chaves são coletadas.

**Quando é utilizado:**
Posicionado no final da fase como ponto de conclusão.

**Componentes:**

* **Sprite:** artefato flutuante com efeito de energia.
* **Colisor:** `BoxCollider2D` (isTrigger).
* **Fonte de Áudio:** som de ativação/portal.
* **Scripts:**

  * `ArtifactActivator.cs`

**Comportamento dos scripts:**

* **`ArtifactActivator`**

  * Verifica se o número de chaves coletadas é suficiente.
  * Ao interação do jogador (`OnTriggerEnter2D`), ativa sequência de final.
  * Reproduz som e animação.
  * Aciona `GameManager.EndGame()` com final normal ou alternativo.

---

#### ☠️ **Prefab: Armadilha**

**Descrição:**
Elemento perigoso que reinicia a fase ao contato.

**Quando é utilizado:**
Distribuído estrategicamente para punir descuido e gerar aprendizado.

**Componentes:**

* **Sprite:** espinhos, buracos, plataformas falsas.
* **Colisor:** `BoxCollider2D` ou `PolygonCollider2D`.
* **Fonte de Áudio:** som de falha/morte.
* **Scripts:**

  * `Trap.cs`

**Comportamento dos scripts:**

* **`Trap`**

  * Detecta colisão com o jogador.
  * Chama `PlayerDeathHandler.Die()`.
  * Pode incluir variação de comportamento (ex.: plataforma que cai).

---

#### 🪧 **Prefab: Sinalização (Orientação Direta)**

**Descrição:**
Elemento visual que fornece orientação explícita ao jogador.

**Quando é utilizado:**
No início de fases e em pontos de bifurcação.

**Componentes:**

* **Sprite:** placa de madeira ou seta direcional.
* **Colisor:** opcional (`BoxCollider2D` se interativo).
* **Fonte de Áudio:** som leve ao aproximar (opcional).
* **Scripts:**

  * `SignHint.cs` *(opcional)*

**Comportamento dos scripts:**

* **`SignHint`**

  * Mostra texto de dica (“Procure as chaves”) ao jogador entrar no trigger.
  * Oculta o texto ao sair da área.
  * Reproduz som sutil de dica.

---

#### 🧩 **Prefab: Plataforma (Orientação Indireta)**

**Descrição:**
Elemento físico do cenário que conduz o jogador por meio do design visual e da física.

**Quando é utilizado:**
Em toda a estrutura da fase, tanto como suporte quanto como armadilha.

**Componentes:**

* **Sprite:** variações de cor e textura conforme tipo.
* **Colisor:** `BoxCollider2D`.
* **Fonte de Áudio:** som de impacto quando o jogador pisa.
* **Scripts:**

  * `FallingPlatform.cs`
  * `VisualHint.cs`

**Comportamento dos scripts:**

* **`FallingPlatform`**

  * Detecta o jogador sobre ela (`OnCollisionEnter2D`).
  * Após breve delay, ativa `Rigidbody2D.gravityScale`.
  * Pode desaparecer após cair e reiniciar após um tempo (caso reutilizável).

* **`VisualHint`**

  * Altera cor, brilho ou textura para indicar confiabilidade (plataforma segura ou falsa).
  * Atua como **orientação indireta**, ensinando o jogador visualmente a identificar perigo.


