# TaskFlow — TP de maintenance WPF

## Contexte

**TaskFlow** est un petit gestionnaire de tâches interne développé dans l'urgence
par un stagiaire aujourd'hui parti. L'application « marche », elle est utilisée
tous les jours par une dizaine de personnes… et le support crible l'équipe de
tickets depuis trois semaines.

Vous reprenez le projet. Votre mission : **reproduire, diagnostiquer et corriger**
les incidents listés plus bas.

Aucun ticket n'indique le fichier en cause. À vous de le trouver

---

## Prérequis et lancement

- **Visual Studio 2022** (17.12 ou plus récent) avec la charge de travail
  *Développement .NET Desktop*
- **SDK .NET 9**
- Aucun package NuGet : le projet se compile hors-ligne
(Nous n'avons pas encore abordé cela durant la formation, NuGet est le gestionnaire de package .NET, l'équivalent de npm avec node.js, ou encore pip en python pour ceux qui connaissent)

```
Ouvrir TaskFlow.sln  ->  Générer la solution  ->  F5
```

La compilation doit passer **sans aucune erreur**, seulement quelques avertissements.

### Données

Les tâches sont stockées dans un fichier JSON :

```
%LOCALAPPDATA%\TaskFlow\tasks.json
```

Au premier lancement, un jeu de **800 tâches** est généré automatiquement
(graine fixe : tout le monde a exactement les mêmes données).
Pour repartir de zéro, supprimez ce fichier et relancez l'application.

---

## Ce que l'application est censée faire

**Fenêtre principale**
- lister les tâches (id, titre, projet, responsable, priorité, statut, échéance, charge)
- rechercher par texte (titre, description, projet, responsable)
- filtrer par statut
- afficher un bandeau de statistiques (total, terminées, en retard, avancement)
- créer, éditer, supprimer une tâche, enregistrer l'ensemble

**Fenêtre d'édition**
- modifier les champs d'une tâche, valider ou annuler

---

## Carnet d'incidents

Sévérité : 🟢 facile · 🟡 moyen · 🔴 difficile

---

### 🟢 TF-001 — Les modifications n'apparaissent pas dans la liste

**Signalé par :** service comptabilité

> « Je double-clique sur une tâche, je change le titre, je valide… et la liste
> affiche toujours l'ancien titre. Si je ferme et je rouvre l'application, le
> nouveau titre est bien là. Pareil pour la priorité et le statut. »

**Reproduction :** double-cliquer sur une ligne, modifier le titre, Valider.

---

### 🟢 TF-002 — La colonne « Échéance » est toujours vide

**Signalé par :** chefs de projet

> « La colonne existe, l'en-tête est là, mais aucune date n'est affichée, sur
> aucune ligne. Pourtant les dates sont bien renseignées : on les voit dans la
> fenêtre d'édition. Aucun message d'erreur. »

**Reproduction :** lancer l'application, regarder la colonne « Échéance ».

---

### 🟢 TF-003 — Les boutons « Éditer » et « Supprimer » restent grisés

**Signalé par :** tout le monde

> « Je sélectionne une ligne, les deux boutons restent gris et ne réagissent
> pas. On a pris l'habitude de double-cliquer pour éditer, mais du coup on ne
> peut plus supprimer une tâche du tout. »

**Reproduction :** cliquer sur une ligne de la liste, observer la barre d'outils.

---

### 🟢 TF-004 — Avancement toujours à 0 %, et fermeture brutale

**Signalé par :** direction

> « Le bandeau affiche "Avancement : 0 %" en permanence, alors qu'il y a
> clairement des tâches terminées dans la liste. »
>
> « Et pire : hier j'ai tapé un mot dans la recherche qui ne donnait aucun
> résultat, et l'application s'est fermée d'un coup, sans rien demander. »

**Reproduction :** (a) observer le bandeau au lancement ;
(b) taper `zzzzz` dans la recherche.

> ⚠️ Deux symptômes, une seule ligne de code en cause.

---

### 🟡 TF-005 — L'application est figée plusieurs secondes

**Signalé par :** accueil

> « Au lancement, la fenêtre reste blanche pendant 4 ou 5 secondes et Windows
> affiche "TaskFlow ne répond pas". Même chose à chaque clic sur Enregistrer :
> tout se bloque, impossible de cliquer ailleurs. »

**Reproduction :** lancer l'application ; cliquer sur « Enregistrer ».

> 💡 Cherchez ce qui est fait pendant ce temps, **et sur quel thread**.
> Cherchez aussi *combien de fois* c'est fait au démarrage.

---

### 🟡 TF-006 — Les nouvelles tâches n'apparaissent pas

**Signalé par :** service achats

> « Je clique sur "Nouvelle", je remplis, je valide : rien ne s'ajoute à la
> liste. Mais si ensuite je tape quelque chose dans la recherche, ma tâche
> apparaît. Et à chaque fois que je fais quoi que ce soit, la liste remonte
> toute seule tout en haut et je perds ma position. »

**Reproduction :** créer une tâche, valider, observer la liste ; puis taper une
lettre dans la recherche.

---

### 🟡 TF-007 — La recherche est inutilisable, la saisie saccade

**Signalé par :** support

> « Quand je tape dans le champ de recherche, les lettres mettent une seconde à
> s'afficher. On tape "facturation", on voit "fac" puis rien, puis tout d'un
> coup le reste. C'est pire à mesure qu'il y a des tâches. »

**Reproduction :** taper rapidement un mot dans le champ de recherche.

---

### 🟡 TF-008 — Ouverture très lente et mémoire énorme

**Signalé par :** service informatique

> « TaskFlow consomme plusieurs centaines de Mo pour afficher une liste de
> texte. Sur les postes anciens, l'affichage de la liste prend un temps fou.
> Il n'y a pourtant que 800 lignes. »

**Reproduction :** ouvrir le gestionnaire des tâches Windows et regarder la
mémoire de TaskFlow après le lancement.

> 💡 Combien de lignes sont réellement *visibles* à l'écran ? Combien sont
> réellement *construites* par WPF ? Regardez comment la liste est imbriquée
> dans le XAML.

---

### 🟡 TF-009 — Plantage à la validation d'une tâche

**Signalé par :** service achats

> « Si je me trompe en tapant la date d'échéance, ou si je laisse la charge
> vide, l'application ne me dit rien : elle se ferme. Je perds tout ce que
> j'avais saisi. »
>
> « Un collègue sur un poste configuré en anglais dit que même une date qui
> nous semble correcte fait planter. »

**Reproduction :** éditer une tâche, saisir `31/02/2026` ou `abc` en échéance,
ou vider le champ des heures, puis Valider.

---

### 🟡 TF-010 — Le défilement saccade et la mémoire grimpe

**Signalé par :** chefs de projet

> « Quand je fais défiler la liste, ça saccade par à-coups réguliers, comme si
> quelque chose se déclenchait en arrière-plan. C'est la colonne Priorité qui
> semble en cause : sans les couleurs c'était fluide avant. »

**Reproduction :** faire défiler la liste de haut en bas plusieurs fois.

> 💡 Combien d'objets sont créés à chaque affichage d'une cellule ? Combien
> devraient l'être ? Regardez du côté des ressources WPF partagées et de
> `Freeze()`.

---

### 🔴 TF-011 — Des pop-ups qui se multiplient

**Signalé par :** service comptabilité

> « Plus je travaille longtemps dans l'application, plus j'ai de fenêtres
> "Les données ont été enregistrées" à fermer. Ce matin j'en ai eu six d'affilée
> pour un seul clic sur Enregistrer. Après redémarrage, c'est reparti à une
> seule. Et l'application devient de plus en plus lente au fil de la journée. »

**Reproduction :** ouvrir puis fermer la fenêtre d'édition 4 fois de suite
(peu importe si l'on valide ou si l'on annule), puis cliquer sur « Enregistrer ».

> 💡 Le nombre de pop-ups vous dit exactement combien d'objets sont encore
> vivants. Pourquoi ne sont-ils pas libérés ?

---

### 🔴 TF-012 — La recherche affiche les résultats d'une autre recherche

**Signalé par :** support

> « Je tape "facturation" assez vite. La liste finit par afficher des lignes qui
> n'ont rien à voir — on dirait le résultat pour "f" ou "fa". Si je rajoute
> puis j'enlève une lettre, ça se remet parfois d'aplomb. Et le compteur en bas
> ne correspond pas non plus à ce que je vois. »

**Reproduction :** taper `facturation` lettre par lettre, très rapidement.
Comparer le contenu de la liste avec le texte du champ de recherche.

> 💡 Plusieurs filtrages peuvent être en vol en même temps. Rien ne garantit
> l'ordre dans lequel ils se terminent, ni lequel gagne.

---

### 🔴 TF-013 — Rien n'est jamais enregistré

**Signalé par :** tout le monde

> « On clique sur Enregistrer, la barre du bas confirme "Données enregistrées
> à 14:32". Le lendemain, on retrouve exactement la liste de départ : toutes nos
> modifications de la veille ont disparu. Aucune erreur, aucun avertissement. »

**Reproduction :** modifier une tâche, cliquer sur « Enregistrer », fermer
l'application, la relancer. Inspecter aussi le fichier
`%LOCALAPPDATA%\TaskFlow\tasks.json`.

> 💡 Deux problèmes se combinent ici : quelque chose qui n'est jamais relâché,
> et quelque chose qui empêche l'erreur de remonter. Corriger un seul des deux
> ne suffira pas.

---

## Consignes de rendu

1. Les projets ne seront pas récupérés, nous échangerons à l'oral sur les différentes solutions proposées et les causes identifiées.

2. Pour chaque ticket, vous devez être capable d'expliquer à l'oral :
   - le **symptôme** observé,
   - la **cause racine** (pas « j'ai changé la ligne 42 »),
   - pourquoi votre correctif la traite **et ne masque pas** le problème.

3. Ne corrigez pas un ticket en supprimant la fonctionnalité.

---

## Bonus — pistes d'amélioration libres

Non demandées, mais c'est là que le projet devient propre :

- MVVM réellement respecté : plus de logique métier dans le code-behind, plus de
  `MessageBox` ni de `Window` instanciée depuis un ViewModel
- Injection du dépôt de données au lieu d'une classe `static`
- `async`/`await` de bout en bout pour les accès au fichier
- `CollectionViewSource` / `ICollectionView` pour le filtre et le tri
- `nameof` à la place des chaînes littérales dans `OnPropertyChanged`
- Validation de saisie (`IDataErrorInfo` ou `INotifyDataErrorInfo`) plutôt que des
  exceptions
- Un projet de tests unitaires sur le filtrage et le calcul des statistiques
