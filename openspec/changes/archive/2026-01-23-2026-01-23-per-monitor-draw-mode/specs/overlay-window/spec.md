# overlay-window Specification Delta

## Added Requirements

### Requirement: Each overlay SHALL be identifiable by its monitor
Each overlay window SHALL store information about which monitor it represents, enabling the system to selectively control mode on a per-monitor basis.

#### Scenario: Overlay stores monitor information
- **GIVEN** an overlay window is being created for a specific monitor
- **WHEN** the overlay is constructed
- **THEN** the overlay stores the monitor's bounds or identifier
- **AND** this information can be used to match the overlay to monitor queries

### Requirement: Overlay mode SHALL be settable independently
Each overlay window SHALL accept mode changes independently of other overlay windows, allowing per-monitor mode control.

#### Scenario: One overlay enters draw mode while others remain in pass-through
- **GIVEN** multiple overlay windows exist for multiple monitors
- **WHEN** the system sets draw mode on one specific overlay
- **THEN** only that overlay enters draw mode
- **AND** other overlays remain in their current mode (pass-through)

#### Scenario: Overlay exits draw mode independently
- **GIVEN** overlay A is in draw mode and overlay B is in pass-through mode
- **WHEN** the system sets pass-through mode on overlay A
- **THEN** overlay A returns to pass-through mode
- **AND** overlay B remains unaffected in pass-through mode
