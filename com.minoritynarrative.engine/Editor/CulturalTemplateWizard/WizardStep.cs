namespace MinorityNarrativeEngine.Editor
{
    /// <summary>
    /// Base class for a single page in the Cultural Template Wizard.
    /// Each step owns its GUI rendering, validation, and state-apply logic.
    /// </summary>
    public abstract class WizardStep
    {
        /// <summary>Reference to the parent wizard for callbacks.</summary>
        public CulturalTemplateWizard wizard;

        /// <summary>Render the step's GUI. Receives the current working context (may be null on step 1).</summary>
        public abstract void OnGUI(CulturalContextBase context);

        /// <summary>
        /// Returns true if the user's input on this step is valid and they can proceed.
        /// Called every frame to enable/disable the Next button.
        /// </summary>
        public abstract bool Validate(CulturalContextBase context);

        /// <summary>
        /// Message shown to the user when Validate() returns false.
        /// </summary>
        public virtual string ValidationMessage => "Please complete all required fields before continuing.";

        /// <summary>
        /// Applies this step's values to the working context and returns it.
        /// On step 1 (SelectContextStep) this creates the initial context instance.
        /// </summary>
        public abstract CulturalContextBase Apply(CulturalContextBase context);
    }
}
