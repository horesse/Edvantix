// ITenanted has been promoted to Edvantix.SharedKernel.SeedWork.ITenanted so all services
// can implement tenant isolation without depending on the Organizations assembly.
// This project-wide alias preserves backward compatibility for all types in this project
// that implement ITenanted — they now implement the SharedKernel version without changes.
global using ITenanted = Edvantix.SharedKernel.SeedWork.ITenanted;
